
IF "%APP_PROJECT%"=="" SET APP_PROJECT=second-shooter

docker compose --project-name %APP_PROJECT% --file docker-compose-cpu.yml build