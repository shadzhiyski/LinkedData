def diet_to_json(diet):
	return {
		'title': diet.title[0],
		'deseases': [d.title[0] for d in diet.has_desease],
		'foods': [f.name for f in diet.has_food]
	}

def desease_to_json(desease):
	return {
		'title': desease.title[0],
		'symptoms': [f.label[0] for f in desease.has_symptom]
	}

DESEASE_SCHEMA = {
	'type': 'object',
	'properties': {
		'title': {'type': 'string'},
		'symptoms': {
			'type': 'array',
			'minItems': 1,
			'uniqueItems': True,
			"items": [
				{
					"type": "string"
				}
			]
		},
	},
	'required': ['title', 'symptoms']
}

DIET_SCHEMA = {
    'type': 'object',
    'properties': {
        'title': {'type': 'string'},
        'deseases': {
			'type': 'array',
			"items": [
				{
					"type": "string"
				}
			]
		},
        'foods': {
			'type': 'array',
			'minItems': 1,
			'uniqueItems': True,
			"items": [
				{
					"type": "string"
				}
			]
		},
    },
    'required': ['title', 'deseases', 'foods']
}