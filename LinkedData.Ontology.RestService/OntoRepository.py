from owlready2 import destroy_entity
import uuid

class OntoRepository():
    def __init__(self, onto):
        self._onto = onto

    def search_one(self, **kvargs):
        return self._onto.search_one(**kvargs)

    def search(self, **kvargs):
        return self._onto.search(**kvargs)

    def create(self, class_name, **kvargs):
        id = uuid.uuid4()

        if 'namespace' in kvargs:
            ns = kvargs['namespace']
            ClassName = getattr(self._onto.get_namespace(ns), class_name)
        else:
            ClassName = getattr(self._onto, class_name)
        
        entity = ClassName(F'{id}')

        return id, entity

    def remove(self, entity):
        destroy_entity(entity)

    def save(self):
        self._onto.save()