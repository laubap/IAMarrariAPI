# IAApi - Detecção de Anomalias para Tags Industriais

## Visão Geral

A IAApi é uma API REST desenvolvida em C# utilizando ASP.NET Core e ML.NET com o objetivo de detectar comportamentos anômalos em tags de sistemas industriais.

A solução foi projetada para integrar-se ao ecossistema do PSI4, permitindo que leituras de sensores sejam analisadas automaticamente através de modelos de Machine Learning.

---

# Objetivos

O projeto tem como objetivo:

* Detectar anomalias em leituras de tags industriais;
* Fornecer uma API simples para integração com sistemas existentes;
* Permitir futura integração com a biblioteca utilizada pelo Viewer do PSI4;
* Possibilitar expansão para múltiplos modelos e tipos de análise.

---

# Arquitetura

Fluxo atual da aplicação:

Cliente / Sistema Externo

↓

POST /api/anomalias/tag

↓

AnomaliaController

↓

PredictionService

↓

IHistoricoService

↓

CsvHistoricoService

↓

CsvFeatureGenerator

↓

AnomalyDetectionService

↓

modelo_tag.zip

↓

Resposta JSON

---

# Componentes

## Program.cs

Responsável por:

* Inicialização da API;
* Configuração dos serviços;
* Registro de dependências;
* Configuração do Swagger;
* Inicialização do modelo de Machine Learning.

---

## AnomaliaController

Responsável por:

* Receber requisições HTTP;
* Validar entrada de dados;
* Encaminhar requisições para o PredictionService;
* Retornar respostas para o cliente.

Endpoint disponível:

```http
POST /api/anomalias/tag
```

---

## PredictionService

Responsável por:

* Orquestrar o fluxo de análise;
* Buscar histórico da tag;
* Gerar features;
* Executar o modelo ML.NET;
* Retornar o resultado da análise.

---

## IHistoricoService

Interface responsável por abstrair a origem dos dados históricos.

Objetivo:

Permitir que a API obtenha histórico de diferentes fontes sem alterar a lógica principal.

---

## CsvHistoricoService

Implementação temporária do IHistoricoService.

Atualmente utiliza arquivos CSV locais para simular dados históricos.

No futuro será substituído por uma implementação baseada na biblioteca do Viewer.

---

## CsvFeatureGenerator

Responsável por transformar leituras brutas em features utilizadas pelo modelo.

Features calculadas:

* Valor Atual
* Média Móvel
* Variação
* Valor Mínimo
* Valor Máximo
* Desvio Padrão

---

## AnomalyDetectionService

Responsável por:

* Treinamento do modelo;
* Salvamento do modelo (.zip);
* Carregamento do modelo;
* Execução de previsões.

Tecnologia utilizada:

* ML.NET
* Randomized PCA

---

# Estrutura do Projeto

```text
IAApi
│
├── Controllers
│   └── AnomaliaController.cs
│
├── Models
│   ├── AnomaliaRequest.cs
│   ├── AnomaliaResponse.cs
│   ├── LeituraBruta.cs
│   ├── SensorData.cs
│   └── SensorPrediction.cs
│
├── Services
│   ├── IHistoricoService.cs
│   ├── CsvHistoricoService.cs
│   ├── CsvFeatureGenerator.cs
│   ├── PredictionService.cs
│   └── AnomalyDetectionService.cs
│
├── Data
│   ├── sensores.csv
│   └── sensores_enriquecido.csv
│
├── ModelsML
│   └── modelo_tag.zip
│
└── Program.cs
```

---

# Exemplo de Requisição

```json
{
  "clienteId": "cliente01",
  "tagName": "TemperaturaTanque01",
  "dataHora": "2026-06-12T14:01:00",
  "valor": 66.5
}
```

---

# Exemplo de Resposta

```json
{
  "clienteId": "cliente01",
  "tagName": "TemperaturaTanque01",
  "ehAnomalia": false,
  "score": 0.15,
  "mensagem": "Comportamento normal na tag TemperaturaTanque01."
}
```

---

# Treinamento do Modelo

Fluxo de treinamento:

```text
sensores.csv
↓
CsvFeatureGenerator
↓
sensores_enriquecido.csv
↓
Treinamento ML.NET
↓
modelo_tag.zip
```

O modelo atualmente é treinado utilizando dados simulados armazenados em CSV.

No futuro, o treinamento será realizado utilizando dados históricos reais obtidos através da biblioteca utilizada pelo Viewer do PSI4.

---

# Evolução Planejada

## Versão Atual

* API REST funcional;
* Modelo de detecção de anomalias por tag;
* Treinamento via CSV;
* Persistência de modelo em ZIP;
* Swagger para testes.

## Próximos Passos

* Integração com biblioteca do Viewer;
* Consulta de histórico em tempo real;
* Treinamento com dados reais;
* Cache de histórico;
* Suporte a múltiplos modelos;
* Detecção de anomalias por processo (multitag);
* Dashboard de monitoramento.

---

# Tecnologias Utilizadas

* .NET 9
* ASP.NET Core
* ML.NET
* Swagger
* C#
* REST API

---

# Autor

Laura Baptistini

Projeto desenvolvido como estudo e prova de conceito para aplicação de Inteligência Artificial em sistemas industriais e monitoramento de tags.
