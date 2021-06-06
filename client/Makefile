
build:
	./node_modules/.bin/webpack --env git-tag=`git describe --always --tags --dirty` --env git-hash=`git rev-list --max-count=1 --no-merges --abbrev-commit HEAD`

run:
	./node_modules/.bin/webpack serve --env dev --no-color

test:
	echo Not implemented.


run-dist:
	npm install --no-save http-server
	./node_modules/.bin/http-server dist -p 32016 
