
version ?= latest

docker:
	docker build -t goodnight:$(version) .

startDocker:
	docker run --rm -it -p 32017:80 --mount type=bind,source=$(shell pwd)/storage,target=/service/storage  goodnight:$(version)

runClient:
	make -C client run

runService:
	make -C service run

countLines:
	cloc --vcs=git --exclude-lang=JSON .
