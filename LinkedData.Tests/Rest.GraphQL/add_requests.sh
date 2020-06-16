
cut -d, -f1 all_unique_genes.csv | head -n10 | tail -n10 | while read gene
do
	echo -e "Inserting gene: ${gene} ..."
	curl --insecure -X POST --header 'Accept: application/json' \
		--header 'Content-Type: application/json' \
		-d "{ \"query\": \"mutation { putGene(gene:{ name: \u0022${gene}\u0022 }) { name } }\" }" \
		'https://127.0.0.1:5000/api/graphql'
	echo -e '\nDone.'
done;