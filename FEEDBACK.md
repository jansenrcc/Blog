# Feedback do Instrutor

#### 28/10/24 - Revisão Inicial - Eduardo Pires

## Pontos Positivos:

- Boa separação de responsabilidades.
- Arquitetura enxuta de acordo com a complexidade do projeto
- Demonstrou conhecimento em Identity e JWT
- Soube aplicar a validação de claims ou proprietario do post
- Mostrou entendimento do ecossistema de desenvolvimento em .NET

## Pontos Negativos:

- A entidade autor não existe, deveria atuar em conjunto com o user (Identity)
- Existem maneiras mais elegantes de obter o usuário do identity e utilizar seus dados.
- A camada "Data" poderia virar "Core" e receber serviços de aplicação para evitar a duplicação e repetição de código comum na API e Web.

## Sugestões:

- Evoluir o projeto para as necessidades solicitadas no escopo sem aumentar a complexidade do projeto.

## Problemas:

- Não consegui executar a aplicação de imediato na máquina. É necessário que o Seed esteja configurado corretamente, com uma connection string apontando para o SQLite.

  **P.S.** As migrations precisam ser geradas com uma conexão apontando para o SQLite; caso contrário, a aplicação não roda.
