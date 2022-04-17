# POC TCC PUC Minas

# Sobre

GSL - Gestão de Serviços de Logística é uma prova de conceito para conclusão do curso Arquitetura de Software Distribuído da PUC Minas

- Técnologias utilizadas no projeto:
  - .net core 5
  - RabbitMQ
  - PostgreSQL
  - Soluções AWS
    - SecretsManager
    - CloudWatch
  - Dapper

# Como rodar o projeto

- Para buildar e subir o app
```
docker-compose build
docker-compose up -d
```

- Para rodar os comandos de dentro da VM:
```
# acessar o bash da aplicação:
docker-compose exec app bash
# acessar o bash do RabbitMQ:
docker-compose exec rabbitmq bash
# acessar o bash do PostgreSQL:
docker-compose exec postgres bash
```

- Para rodar as migrations:
```

```


- Para popular a base:
```

```

- Para acessar a console do RabbitMQ:
```
http://localhost:15672/
user: radmin
pass: radmin
```

- Para acessar o Swagger:
```
http://localhost/
```

- Para acessar o Kong:
```
http://localhost:8001/
```

- Para acessar o Konga:
```
http://localhost:1337/
```

- Para rodar os testes:
```

```