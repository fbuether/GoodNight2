#!/bin/sh -xm

sudo echo "requesting sudo privs, to properly launch sub-process..."

if [ ! -f  perfcollect ]
then
	curl -OL http://aka.ms/perfcollect
	chmod +x ./perfcollect
	sudo ./perfcollect install
fi

make build

sudo ./perfcollect collect testSession &
sudoprocid=$!
sleep 1
echo `ps --ppid $sudoprocid -o pid=`
perfprocid=`ps --ppid $sudoprocid -o pid=`
echo $perfprocid

make test-perf

fg

# sudo kill -INT $perfprocid
# wait $perfprocid
# sudo chown fbuether:fbuether testSession.trace.zip


sudo ./perfcollect view testSession
