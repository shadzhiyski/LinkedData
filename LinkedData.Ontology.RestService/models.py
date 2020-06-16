from owlready2 import *

def load_models(onto:Ontology):
    with onto:
        class Diet(Thing):
            pass

        class Desease(Thing):
            pass
        
        class has_symptom(Property):
            domain=[Desease]
            range=[onto.get_namespace('http://purl.obolibrary.org/obo/DDO.owl').OGMS_0000020]
        
        class has_desease(Property):
            domain=[Diet]
            range=[Desease]

        class has_food(Property):
            domain=[Diet]
            range=[onto.get_namespace('http://www.owl-ontologies.com/OntoFood.owl').Food]