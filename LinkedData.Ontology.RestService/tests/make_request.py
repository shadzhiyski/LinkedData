import requests
import urllib

# r_des = requests.post(
#     url='http://localhost:5555/ontology/desease',
#     headers={
#         "Content-Type": "application/json"
#     },
#     json={
#         'title': 'Diebetes Type 2',
#         'symptoms': [
#             'weight loss',
#             'fatigue',
#             'blurred vision',
#             'delayed wound healing'
#         ]
#     }
# )
# # TODO: dsffdgd
# r = requests.post(
#     url='http://localhost:5555/ontology/diet',
#     headers={
#         "Content-Type": "application/json"
#     },
#     json={
#         'title': 'Ketogenic Diet',
#         'deseases': [
#             'Diebetes Type 2'
#         ],
#         'foods': [
#             'Fishery_products',
#             'Cheeses',
#             'Eggs',
#             'Milk',
#             'Yogurt'
#         ]
#     }
# )

# TODO: dsffdgd
with open('tmp/DiabetesPatientData/_4VR0NNIZS.converted.modified.xml', 'r', encoding='utf-8') as fxml:
    xml_txt = '\n'.join(fxml.readlines())
    
r = requests.post(
    url='http://localhost:5555/ontology/patient/import',
    headers={
        "Content-Type": "text/xml"
    },
    data=xml_txt.encode('utf-8')
)

print(r.text)