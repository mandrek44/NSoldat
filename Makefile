DOTNET 		?= dotnet
TOPTARGETS 	:= all run

SUBDIRS := "NSoldat.StatsHarvester"

$(TOPTARGETS): $(SUBDIRS)
$(SUBDIRS):
	$(MAKE) -C $@ $(MAKECMDGOALS)

.PHONY: $(TOPTARGETS) $(SUBDIRS)

export DOTNET