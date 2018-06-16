# NSoldat

.NET Library and Tools for communicating with Soldat server. 

## Stats Harvester
Connects to specified soldat server and parses all incoming events. Each event data is extended with [REFRESH](https://wiki.soldat.pl/index.php/Refresh) packet information. Data is stored in "output.json" file.

# CLI
Prerequisites: GNU Make.

Build:

    make build

Run stats harvester:

    make run

Run tests:

    make test


# Roadmap

 - Multi server console: send commands to multiple server
 - Tournament runner: control multiple soldat servers according to predefined scenario
 - Harvester service: run harvester as Windows Service
 - Statistics: compile harvester data into statistics (players, frags, points, weapons etc...)
 - Web UI: Create web ui for statistics and multi-server console
