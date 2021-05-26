default: build

help: ## Help
	@grep -E '^[a-zA-Z_-]+:.*?##.*$$' $(MAKEFILE_LIST) | sort | awk '{split($$0, a, ":"); printf "\033[36m%-30s\033[0m %-30s %s\n", a[1], a[2], a[3]}'

build: ## Build project.
	dotnet build -r win-x64 -c Release
	dotnet build -r linux-x64 -c Release