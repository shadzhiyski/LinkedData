import json

from flask import Flask
from flask import session, request
from flask import jsonify
from flask import abort
from flask_expects_json import expects_json

app = Flask(__name__, template_folder='templates')
app.debug = True
app.config['SERVER_NAME'] = 'localhost:5555'

from OntoRepository import OntoRepository

from owlready2 import *
onto_path.append("Repository")
onto = get_ontology('file://Repository/DMTO.owl')
onto.load()

from json_utils import *

from models import load_models
load_models(onto)

repo = OntoRepository(onto)

@app.route('/ontology/diet/suggest', methods=['GET'])
def suggest_diet():
	abort(501)
	# TODO: implement diets suggestion
	...

@app.route('/ontology/diet', methods=['GET'])
def get_diet():
	
	title = request.args.get('title')

	if not title:
		abort(400)
	
	diet_onto = repo.search_one(is_a=onto.Diet, title=title)

	if not diet_onto:
		abort(404)

	return jsonify(diet_to_json(diet_onto))

@app.route('/ontology/diet', methods=['POST'])
@expects_json(DIET_SCHEMA)
def create_diet():

	diet_json = request.json

	deseases = [desease for desease in [repo.search_one(title=desease) \
			for desease in diet_json['deseases']
		] if desease is not None
	]

	foods = [food for food in [
			repo.search_one(iri=F'*{food}') for food in diet_json['foods']
		] if food is not None
	]

	id, diet_onto = repo.create(class_name='Diet')

	diet_onto.title = diet_json['title']
	diet_onto.has_desease = deseases
	diet_onto.has_food = foods

	repo.save()

	return jsonify({
		'id': F'{id}',
		'diet': diet_to_json(diet_onto)
	})

@app.route('/ontology/desease', methods=['GET'])
def get_desease():
	
	title = request.args.get('title')

	if not title:
		abort(400)
	
	desease_onto = repo.search_one(is_a=onto.Desease, title=title)

	if not desease_onto:
		abort(404)

	return jsonify(desease_to_json(desease_onto))

@app.route('/ontology/desease', methods=['POST'])
@expects_json(DESEASE_SCHEMA)
def create_desease():

	desease_json = request.json

	symptoms = [repo.search_one(label=symptom) \
		for symptom in desease_json['symptoms']
	]

	id, desease_onto = repo.create(class_name='Desease')

	desease_onto.title = desease_json['title']
	desease_onto.has_symptom = symptoms

	repo.save()

	return jsonify({
		'id': F'{id}',
		'desease': desease_to_json(desease_onto)
	})

from lxml import objectify
import lxml

from flask import Response
@app.route('/ontology/patient', methods=['GET'])
def get_patient():
	patient_profile_id = request.args.get('patient_profile_id')

	if not patient_profile_id:
		abort(400)

	repo.search_one(is_a=onto.DMTO_0001670, iri=F'*{patient_profile_id}')

	return jsonify({ 'reference_id': str(patient_profile_id) })

@app.route('/ontology/patient/import', methods=['POST'])
def import_patients():
	patient_xml_content = request.data
	root = objectify.fromstring(patient_xml_content)

	mappings = {
		'Lymph': 'DDO_0000259',
		'Mid': 'DDO_0000260',
		'MCH': 'DDO_0000255',
		'MCHC': 'DDO_0000256',
		'MCV': 'DDO_0000254',
		'HCT': 'DDO_0000253',
		'RBC': 'DDO_0000251',
		
		'TotalCholesterol': 'DDO_0000274',

		'HGB': 'DDO_0009214'
	}

	for input_amb_list in root.Doctor.AmbList:
		id, patient_profile = repo.create(
			class_name='DMTO_0001670'
		)

		patient_profile.has_lab_test = []

		input_anamnesa = input_amb_list.Anamnesa

		# lab_tests = []
		for name, cls_name in mappings.items():
			item_id, item_instance = repo.create(
				class_name='DDO_00906290',#cls_name,
				namespace='http://purl.obolibrary.org/obo/DDO.owl'
			)

			item_instance.has_date = F'{input_amb_list.dataAl}T{input_amb_list.time}Z'
			item_instance.has_quantitative_Value = input_anamnesa['Blood'][name].text

			patient_profile.has_lab_test.append(item_instance)

		# patient_profile.has_lab_test = lab_tests
	
	repo.save()

	return jsonify({ 'reference_id': str(id) })
	# jsonify([amb_list.Patient.EGN.text for amb_list in root.Doctor.AmbList])
	# Response(str(root.Doctor.AmbList), mimetype='text/xml')


if __name__ == '__main__':
    app.run()