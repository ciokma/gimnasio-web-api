docker build -t ciokma/gimnasio-web-api -f Dockerfile .
docker run -p 5211:5211 --name gimnasio-api ciokma/gimnasio-web-api
# inyectar con variables de ambientes 
# correr contenedor
docker run --env-file .env -p 5211:5211 --name gimnasio-api ciokma/gimnasio-web-api

docker logs gimnasio-api
docker ps -a
docker login
docker push ciokma/gimnasio-web-api

