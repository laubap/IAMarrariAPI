# IAApi - Detecção de Anomalias para Tags Industriais

## Visão Geral

A IAApi é uma API desenvolvida em .NET para detecção de anomalias em tags industriais utilizando análise comportamental baseada em histórico real de operação.

O sistema integra-se ao PowerServer através de um Bridge responsável pela obtenção dos dados históricos das tags. A partir desses dados, a API realiza limpeza automática de outliers, cria perfis estatísticos individuais para cada tag e utiliza esses perfis para identificar comportamentos anormais.

---

## Arquitetura da Solução

PowerServer

↓

Bridge (HTTP)

↓

IAApi

↓

Busca de Histórico

↓

Limpeza de Outliers (IQR)

↓

Treinamento de Perfil

↓

Banco SQLite

↓

Detecção de Anomalias

---

## Tecnologias Utilizadas

* .NET 10
* ASP.NET Core
* Entity Framework Core
* SQLite
* Swagger/OpenAPI
* PowerServer
* HTTP Listener
* JSON

---

## Principais Funcionalidades

### Cadastro de Tags para Monitoramento

Permite configurar quais tags terão monitoramento inteligente habilitado.

### Busca de Histórico Real

Consulta o histórico diretamente do PowerServer através do Bridge.

### Limpeza Automática de Outliers

Remove valores extremamente fora do padrão utilizando o método IQR (Interquartile Range).

### Treinamento de Perfil

Cria um perfil estatístico específico para cada tag.

Informações calculadas:

* Média
* Desvio padrão
* Valor mínimo
* Valor máximo
* Amplitude
* Percentual de zeros
* Variação média
* Quantidade de picos
* Quantidade de outliers removidos

### Retreinamento de Perfil

Atualiza o perfil da tag utilizando dados mais recentes.

### Listagem de Perfis

Permite consultar todos os perfis treinados.

### Detecção de Anomalias

Compara novos valores recebidos com o perfil da tag e identifica desvios significativos.

---

## Banco de Dados

### Tabela: TagsIaConfig

Responsável pelas configurações das tags monitoradas.

Campos:

* Id
* ClienteId
* TagName
* IaAtiva
* DataConfiguracao

---

### Tabela: PerfisIa

Armazena os perfis comportamentais treinados.

Campos:

* Id
* ClienteId
* TagName
* Media
* DesvioPadrao
* Minimo
* Maximo
* Amplitude
* PercentualZeros
* VariacaoMedia
* QuantidadePicos
* TotalRegistrosHistorico
* TotalRegistrosUsados
* TotalOutliersRemovidos
* DataTreinamento

---

## Fluxo de Treinamento

1. Usuário informa a tag.
2. A API consulta o histórico no Bridge.
3. O histórico é validado.
4. Outliers são removidos utilizando IQR.
5. Estatísticas do perfil são calculadas.
6. O perfil é salvo no banco de dados.
7. O perfil fica disponível para futuras detecções.

---

## Fluxo de Detecção de Anomalias

1. Um novo valor é recebido.
2. A API localiza o perfil da tag.
3. Os limites aceitáveis são calculados:

Limite Inferior = Média - (3 × Desvio Padrão)

Limite Superior = Média + (3 × Desvio Padrão)

4. O valor recebido é comparado com os limites.
5. A API retorna se o valor representa ou não uma anomalia.

---

## Endpoints

### Treinar Perfil

POST

/api/tags/treinar-perfil

---

### Retreinar Perfil

POST

/api/tags/retreinar-perfil

---

### Listar Perfis

GET

/api/tags/perfis

---

### Detectar Anomalia

POST

/api/anomalias/tag

---

## Configuração do Bridge

Arquivo:

appsettings.json

```json
{
  "Bridge": {
    "BaseUrl": "http://localhost:5001",
    "HorasHistorico": 24
  }
}
```

---

## Execução

### Executar o Bridge

```bash
PowerServerCurrentTagsPropGet.exe
```

### Executar a API

```bash
dotnet run
```

### Swagger

```text
http://localhost:5145/swagger
```

---

## Melhorias Futuras

* Transformar o Bridge em Serviço Windows
* Retreinamento automático agendado
* Dashboard de monitoramento
* Notificações por e-mail
* Agrupamento automático de tags por comportamento
* Implementação de Machine Learning não supervisionado
* Histórico de anomalias detectadas
* Implementação com o software da Marrari
* Cliente obtido através do usuário autentificado no sistema

---

## Autor

Laura Baptisini

Projeto desenvolvido para aplicação de Inteligência Artificial em ambientes industriais utilizando PowerServer e .NET.
