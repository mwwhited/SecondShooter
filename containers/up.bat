
IF "%APP_PROJECT%"=="" SET APP_PROJECT=2ndShooter

docker compose --project-name %APP_PROJECT% --file docker-compose-cpu.yml up --detach 