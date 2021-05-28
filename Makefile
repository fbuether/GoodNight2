
docker:
	docker build -t goodnight:2.0.0-dev .

startDocker:
	docker run --rm -it -p 32017:80 --mount type=bind,source=$(shell pwd)/storage,target=/service/storage  goodnight:2.0.0-dev

runClient:
	make -C client run

runService:
	make -C service run

countLines:
	cloc --vcs=git --exclude-lang=JSON .
