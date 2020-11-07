import json

from flask import Flask
from flask import session, request
from flask import jsonify
from flask import abort
from flask_expects_json import expects_json

from lxml import objectify, etree as ET

from DmtoRepository import DmtoRepository

app = Flask(__name__, template_folder='templates')
app.debug = True
app.config['SERVER_NAME'] = 'localhost:5555'

lab_tests_map = {
	'TotalCholesterol': 'DDO.:DDO_0000274',
	'Glucose': 'DDO.:DDO_0000244',
	'UricAcid': 'DDO.:DDO_0000296'
}

def import_patients_data(ontology_repo, patients_xml):
	patients = []
	for input_amb_list in patients_xml.Doctor.AmbList:
		patient = ontology_repo.add_patient()
		patient_profile = ontology_repo.add_patient_profile()
		treatment_plan = ontology_repo.add_treatment_plan()
		lifestyle_subplan = ontology_repo.add_lifestyle_subplan()
		diet = ontology_repo.add_diet()
		breakfast_meal = ontology_repo.add_breakfast_meal()

		ontology_repo.add_data_property(
			property_owner=patient_profile,
			property_name='https://bioportal.bioontology.org/ontologies/DTO.owl#DTO:0002111', # has_total_calories
			data_type='http://www.w3.org/2001/XMLSchema#double',
			data_value='1700',
		)

		ontology_repo.add_relation(
			property_owner=patient,
			reference_to=patient_profile,
			property_name='#DMTO_0001667') # has_patient_profile
		ontology_repo.add_relation(
			property_owner=patient_profile,
			reference_to=treatment_plan,
			property_name='#DMTO_0001671') # has_treatment_plan
		ontology_repo.add_relation(
			property_owner=treatment_plan,
			reference_to=lifestyle_subplan,
			property_name='#DMTO_0001701') # has_part
		ontology_repo.add_relation(
			property_owner=lifestyle_subplan,
			reference_to=diet,
			property_name='https://bioportal.bioontology.org/ontologies/DTO.owl#DTO:0001953') # has_lifestyle_participant
		ontology_repo.add_relation(
			property_owner=diet,
			reference_to=breakfast_meal,
			property_name='https://bioportal.bioontology.org/ontologies/DTO.owl#DTO:0001972') # has_breakfast_meal

		anamnesa = input_amb_list.Anamnesa
		for lab_test_label, lab_test_class in lab_tests_map.items():
			lab_test = ontology_repo.add_lab_test(lab_test_class, lab_test_label)

			value = anamnesa['Blood'][lab_test_label].text
			ontology_repo.add_data_property(
				property_owner=lab_test,
				property_name='DDO.:DDO_0000134', # has_quantitative_Value
				data_type='http://www.w3.org/2001/XMLSchema#float',
				data_value=value,
				is_abbreviated=True
			)

			ontology_repo.add_relation(patient_profile, lab_test, 'DDO.:DDO_0000311', True)

		patients.append(patient)

	return patients

@app.route('/ontology/patient/import', methods=['POST'])
def import_patients():
	patients_xml_content = request.data
	patients_xml = objectify.fromstring(patients_xml_content)

	dmto_ontology_repo = DmtoRepository('Repository/DMTO_converted.owl')

	patients = import_patients_data(dmto_ontology_repo, patients_xml)

	dmto_ontology_repo.save()

	return jsonify({ 'patients_ids': patients })


if __name__ == '__main__':
    app.run()