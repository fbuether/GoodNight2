
docker:
	docker build -t goodnight:dev .

version=$(shell git describe --always --tags)
hash=$(shell git rev-list --max-count=1 --no-merges --abbrev-commit HEAD)

docker-tag:
	git checkout main
	git pull
	echo checking out version $(version)
	git checkout $(version)
	docker build -t goodnight:$(version) -t goodnight:latest --build-arg GIT_TAG=$(version) --build-arg GIT_HASH=$(hash) .

startDocker:
	docker run --rm -it -p 32017:80 --mount type=bind,source=$(shell pwd)/storage,target=/service/storage goodnight:$(version)

runClient:
	make -C client run

runService:
	make -C service run

countLines:
	cloc --vcs=git --exclude-lang=JSON .
