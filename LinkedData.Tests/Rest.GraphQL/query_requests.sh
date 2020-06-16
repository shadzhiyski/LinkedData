# GraphQL tests

# Schema info test
# GET: schema info
curl --insecure -X GET --header 'Accept: application/json' \
	'https://127.0.0.1:5000/api/graphql'

# Queries tests
# POST: get gene
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B gene(name:\u0022BRCA1\u0022) \u007B name \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# POST: get all genes
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B genes \u007B name \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# POST: get all proteins
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B proteins \u007B name \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# POST: get all proteins (by gene)
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B proteins(geneName:\u0022BRCA1\u0022) \u007B name \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# POST: get genes with proteins
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B genesProteins \u007B name, proteins \u007B name \u007D \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# POST: get gene with proteins
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B genesProteins(geneName: \u0022BRCA1\u0022) \u007B name, proteins \u007B name \u007D \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# POST: get proteins with descriptions
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "\u007B proteinsDescriptions(geneName: \u0022BRCA1\u0022) \u007B name, descriptions \u007B text, type \u007D \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'

# Mutations tests
# POST: get proteins with descriptions
curl --insecure -X POST --header 'Accept: application/json' \
	--header 'Content-Type: application/json' \
	-d '{ "query": "mutation \u007B putGene(gene:\u007B name: \u0022BRCA2\u0022 \u007D) \u007B name \u007D \u007D" }' \
	'https://127.0.0.1:5000/api/graphql'