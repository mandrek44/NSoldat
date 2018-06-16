DOTNET 		?= dotnet

all: build
	
restore:
	$(DOTNET) restore

build: 
	$(DOTNET) build

run: build
	$(DOTNET) run

.PHONY: restore build run