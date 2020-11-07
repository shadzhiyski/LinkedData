import requests
import urllib

with open('data/_4VR0NNIZS.converted.modified.xml', 'r', encoding='utf-8') as fxml:
    xml_txt = '\n'.join(fxml.readlines())

r = requests.post(
    url='http://localhost:5555/ontology/patient/import',
    headers={
        "Content-Type": "text/xml"
    },
    data=xml_txt.encode('utf-8')
)

print(r.text)