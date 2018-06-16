DOTNET 		?= dotnet
TOPTARGETS 	:= all run

SUBDIRS := "NSoldat.StatsHarvester"

$(TOPTARGETS): $(SUBDIRS)
$(SUBDIRS):
	$(MAKE) -C $@ $(MAKECMDGOALS)

test: NSoldat.Tests
NSoldat.Tests: 
	$(MAKE) -C $@ $(MAKECMDGOALS)

.PHONY: $(TOPTARGETS) $(SUBDIRS) test NSoldat.Tests

export DOTNET