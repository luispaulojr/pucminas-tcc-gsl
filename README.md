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
docker-compose exec web bash
```

- Para rodar as migrations:
```

```


- Para popular a base:
```

```

- Para acessar o RabbitMQ:
```
http://localhost:15672/
```

- Para rodar os testes:
```

```