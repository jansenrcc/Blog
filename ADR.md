# ADR

## Pauta : Design camada "data"
Após a criação da camada "Data" foi percebido a necessidade de migrar serviços em comum entre API e WEB (valdiações) para essa camada, portanto seria necessário alterar o nome do projeto de .Data para .Core, o que iria contra o que foi definido como template na documentação de requisitos informada.

### Decisão
Optei por manter o desgin .Data para respeitar o template definido nos requisitos e criar uma organização de "Core" dentro dentro do projeto "Data".
O Core carregará as validações em comum entre os projetos.

   
### Consequências
- **Positivas**: Respeitar o template definido nos requisitos.
- **Negativas**: Design "estranho".

