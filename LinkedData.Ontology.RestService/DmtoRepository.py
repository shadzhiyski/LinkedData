from lxml import objectify, etree as ET
import uuid

class DmtoRepository():
    def __init__(self, ontology_path):
        self._ontology_path = ontology_path
        self._ontology_xml = ET.parse(ontology_path).getroot()

    def add_patient(self):
	    return self._add_individual(class_name='#DMTO_0000021', prefix='#PATIENT')

    def add_patient_profile(self):
        return self._add_individual(class_name='#DMTO_0001670', prefix='#PATIENT_PROFILE')

    def add_lab_test(self, class_name, test_name):
        return self._add_individual(class_name=class_name, prefix=F'#LAB_TEST_{test_name}', is_abbreviated=True)

    def add_treatment_plan(self):
        return self._add_individual(class_name='#DMTO_0000044', prefix='#TREATMENT_PLAN')

    def add_lifestyle_subplan(self):
        return self._add_individual(class_name='#DMTO_0001712', prefix='#LIFESTYLE_SUBPLAN')

    def add_diet(self):
        return self._add_individual(class_name='#DMTO_0001704', prefix='#DIET')

    def add_breakfast_meal(self):
        return self._add_individual(class_name='Onto:meal', prefix='#BREAKFAST_MEAL', is_abbreviated=True)

    def add_relation(self, property_owner, reference_to, property_name, is_abbreviated=False):
        object_property_assertion = ET.Element('ObjectPropertyAssertion')

        if is_abbreviated:
            object_property_assertion.append(ET.Element('ObjectProperty', abbreviatedIRI=F'{property_name}'))
        else:
            object_property_assertion.append(ET.Element('ObjectProperty', IRI=F'{property_name}'))

        object_property_assertion.append(ET.Element('NamedIndividual', IRI=property_owner))
        object_property_assertion.append(ET.Element('NamedIndividual', IRI=reference_to))

        self._ontology_xml.append(object_property_assertion)

        return object_property_assertion

    def add_data_property(self, property_owner, property_name, data_type, data_value, is_abbreviated=False):
        object_property_assertion = ET.Element('DataPropertyAssertion')

        if is_abbreviated:
            object_property_assertion.append(ET.Element('DataProperty', abbreviatedIRI=F'{property_name}'))
        else:
            object_property_assertion.append(ET.Element('DataProperty', IRI=F'{property_name}'))

        object_property_assertion.append(ET.Element('NamedIndividual', IRI=property_owner))

        literal = ET.Element('Literal', datatypeIRI=data_type)
        literal.text = data_value
        object_property_assertion.append(literal)

        self._ontology_xml.append(object_property_assertion)

        return object_property_assertion

    def save(self):
        self._ontology_xml[:] = sorted(self._ontology_xml, key=lambda x: 'A' if x.tag.endswith('Prefix') else x.tag)

        et = ET.ElementTree(self._ontology_xml)
        et.write(self._ontology_path, pretty_print=True)

    def _add_individual(self, class_name, prefix, is_abbreviated=False):
        declaration = ET.Element('Declaration')
        class_assertion = ET.Element('ClassAssertion')

        if is_abbreviated:
            class_assertion.append(ET.Element('Class', abbreviatedIRI=F'{class_name}'))
        else:
            class_assertion.append(ET.Element('Class', IRI=F'{class_name}'))

        id = uuid.uuid4()
        individual_name = F'{prefix}_{id}'
        declaration.append(ET.Element('NamedIndividual', IRI=individual_name))
        class_assertion.append(ET.Element('NamedIndividual', IRI=individual_name))

        self._ontology_xml.append(declaration)
        self._ontology_xml.append(class_assertion)

        return individual_name