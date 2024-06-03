#!/bin/bash

if [ -z "$APP_PROJECT" ]; then
  APP_PROJECT="2ndShooter"
fi

docker compose --project-name "$APP_PROJECT" --file docker-compose-cuda.yml up --detach
