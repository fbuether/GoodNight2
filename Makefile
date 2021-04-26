
runClient:
	make -C client run

runService:
	make -C service run

countLines:
	cloc --vcs=git --exclude-lang=JSON .
