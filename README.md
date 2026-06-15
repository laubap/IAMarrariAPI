# IA Marrari API

## Sobre o Projeto

A IA Marrari API é uma API desenvolvida em ASP.NET Core utilizando ML.NET para detecção de anomalias em dados industriais.

O objetivo da solução é permitir que sistemas supervisórios, historiadores e plataformas de monitoramento identifiquem automaticamente comportamentos anormais em tags de processo, auxiliando operadores e equipes de manutenção na tomada de decisão.

Atualmente o projeto possui modelos específicos para diferentes categorias de tags:

* Temperatura
* Pressão
* Corrente

Cada categoria possui seu próprio modelo de Machine Learning treinado de forma independente.

---

## Arquitetura Atual

Fluxo de funcionamento:

```text
Cliente
↓
Seleciona Tag
↓
Informa Tipo da Tag
↓
API busca histórico
↓
Calcula Features
↓
Modelo correspondente
↓
Detecção de Anomalia
↓
Resposta
```

Exemplo:

```text
TipoTag = temperatura
↓
modelo_temperatura.zip

TipoTag = pressao
↓
modelo_pressao.zip

TipoTag = corrente
↓
modelo_corrente.zip
```

---

## Tecnologias Utilizadas

* .NET 10
* ASP.NET Core Web API
* ML.NET
* Swagger
* C#

---

## Estrutura do Projeto

```text
IAApi
│
├── Controllers
│   └── AnomaliaController.cs
│
├── Services
│   ├── PredictionService.cs
│   ├── CsvHistoricoService.cs
│   ├── CsvFeatureGenerator.cs
│   └── AnomalyDetectionService.cs
│
├── Models
│   ├── SensorData.cs
│   ├── SensorPrediction.cs
│   ├── AnomaliaRequest.cs
│   └── AnomaliaResponse.cs
│
├── Data
│   ├── temperatura.csv
│   ├── pressao.csv
│   └── corrente.csv
│
└── ModelsML
    ├── modelo_temperatura.zip
    ├── modelo_pressao.zip
    └── modelo_corrente.zip
```

---

## Features Calculadas

Antes de realizar a previsão, o sistema gera automaticamente características estatísticas a partir do histórico da tag.

Atualmente são calculadas:

* Valor Atual
* Média Móvel
* Variação
* Valor Mínimo da Janela
* Valor Máximo da Janela
* Desvio Padrão

Essas informações são utilizadas como entrada para o modelo de Machine Learning.

---

## Treinamento dos Modelos

Cada modelo é treinado utilizando históricos pertencentes à sua categoria.

Exemplos:

### Modelo de Temperatura

Treinado com históricos de tags de temperatura.

Arquivo gerado:

```text
ModelsML/modelo_temperatura.zip
```

### Modelo de Pressão

Treinado com históricos de tags de pressão.

Arquivo gerado:

```text
ModelsML/modelo_pressao.zip
```

### Modelo de Corrente

Treinado com históricos de tags de corrente.

Arquivo gerado:

```text
ModelsML/modelo_corrente.zip
```

---

## Endpoint Principal

### Detectar Anomalia

```http
POST /api/anomalias/tag
```

Exemplo de requisição:

```json
{
  "clienteId": "cliente01",
  "tagName": "PT100_Tanque01",
  "tipoTag": "temperatura",
  "dataHora": "2026-06-12T14:01:00",
  "valor": 80.0
}
```

Exemplo de resposta:

```json
{
  "clienteId": "cliente01",
  "tagName": "PT100_Tanque01",
  "tipoTag": "temperatura",
  "ehAnomalia": true,
  "score": 0.87,
  "mensagem": "Anomalia detectada na tag PT100_Tanque01."
}
```

---

## Próximos Passos

A versão atual utiliza arquivos CSV para simular históricos de processo.

A evolução prevista para o projeto é a integração com a biblioteca do Viewer, permitindo:

* Consulta de histórico real das tags
* Busca automática de dados históricos
* Treinamento com dados reais de processo
* Classificação automática de tags por tipo ou unidade de engenharia
* Aprimoramento contínuo dos modelos de Machine Learning

---

## Autor

Laura Marrari Baptístini

Projeto desenvolvido para estudo e aplicação de Inteligência Artificial em monitoramento industrial.
