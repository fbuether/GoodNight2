
docker:
	docker build -t goodnight:dev .

docker-tag:
	git checkout main
	git pull --prune --tags --all
	echo checking out version:
	git describe --always --tags
	git checkout $(shell git describe --always --tags)
	docker build -t goodnight:$(shell git describe --always --tags) -t goodnight:latest --build-arg GIT_TAG=$(shell git describe --always --tags) --build-arg GIT_HASH=$(shell git rev-list --max-count=1 --no-merges --abbrev-commit HEAD) .

startDocker:
	docker run --rm -it -p 32017:80 --mount type=bind,source=$(shell pwd)/storage,target=/service/storage goodnight:$(shell git describe --always --tags)

runClient:
	make -C client run

runService:
	make -C service run

countLines:
	cloc --vcs=git --exclude-lang=JSON .
