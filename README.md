# PBL-PROJECT BASED LEARNING

## Introdução

Aplicação de IoT no monitoramento de estufas utilizadas na construção de motores elétricos.

O projeto visa implementar um sistema de controle e monitoramento IoT para as estufas de secagem de motores elétricos, utilizando a plataforma de back-end FIWARE para realizar o processamento e armazenamento das informações de contexto, visando otimizar o processo global de fabricação, assegurando precisão na regulação de temperatura, monitoramento remoto em tempo real e aprimoramento da eficiência operacional, resultando em motores elétricos de alta qualidade e consistência em todas as unidades da empresa. O sistema proposto será apoiado por uma plataforma na Web desenvolvida em Asp.net MVC que dará suporte aos cadastros com exibição dos dados no formato de consultas e dashboards

## Arquitetura

![FiwareDeploy_new drawio](https://github.com/Hugo-Victorr/PBL-Project/assets/78647874/2ca611ed-d5f2-4ec2-8b85-ed7b00d5fb1d)

## Código C++ ESP32

Código implementado no ESP32 para o monitoramento da temperatura em uma estufa de motores elétricos:

```cpp
#include <WiFi.h>
#include <PubSubClient.h>

// Configurações - variáveis editáveis
const char* default_SSID = "S20FEMatheus"; // Nome da rede Wi-Fi
const char* default_PASSWORD = "galinha01"; // Senha da rede Wi-Fi
const char* default_BROKER_MQTT = "172.173.173.47"; // IP do Broker MQTT
const int default_BROKER_PORT = 1883; // Porta do Broker MQTT
const char* default_TOPICO_SUBSCRIBE = "/TEF/TempSensor01/cmd"; // Tópico MQTT de escuta
const char* default_TOPICO_PUBLISH_2 = "/TEF/TempSensor01/attrs/t"; // Tópico MQTT de envio de informações para Broker
const char* default_ID_MQTT = "fiware_001"; // ID MQTT
// Declaração da variável para o prefixo do tópico
const char* topicPrefix = "TempSensor01";

// Variáveis para configurações editáveis
char* SSID = const_cast<char*>(default_SSID);
char* PASSWORD = const_cast<char*>(default_PASSWORD);
char* BROKER_MQTT = const_cast<char*>(default_BROKER_MQTT);
int BROKER_PORT = default_BROKER_PORT;
char* TOPICO_SUBSCRIBE = const_cast<char*>(default_TOPICO_SUBSCRIBE);
char* TOPICO_PUBLISH_2 = const_cast<char*>(default_TOPICO_PUBLISH_2);
char* ID_MQTT = const_cast<char*>(default_ID_MQTT);

WiFiClient espClient; // Cria um cliente WiFi
PubSubClient MQTT(espClient); // Cria um cliente MQTT usando o cliente WiFi
char EstadoSaida = '0'; // Variável para armazenar o estado de saída

void initSerial() {
    Serial.begin(115200); // Inicializa a comunicação serial a 115200 bps
}

// Início da conexão wifi do ESP 
void initWiFi() {
    delay(10); // Pequena pausa
    Serial.println("------Conexao WI-FI------");
    Serial.print("Conectando-se na rede: ");
    Serial.println(SSID);
    Serial.println("Aguarde");
    reconectWiFi(); // Tenta conectar ao WiFi
}

// Início da comunicação MQTT
void initMQTT() {
    MQTT.setServer(BROKER_MQTT, BROKER_PORT); // Configura o servidor MQTT
}

void setup() {
    initSerial(); // Inicializa a comunicação serial
    initWiFi(); // Conecta ao WiFi
    initMQTT(); // Configura o MQTT
    delay(5000); // Aguarda 5 segundos
}

// Loop de execução do ESP-32
void loop() {
    VerificaConexoesWiFIEMQTT(); // Verifica as conexões WiFi e MQTT
    handleTemp(); // Lê e publica a temperatura
    MQTT.loop(); // Mantém o cliente MQTT ativo
    delay(2000); // Aguarda 2 segundos
}

void reconectWiFi() {
    if (WiFi.status() == WL_CONNECTED)
        return; // Se já está conectado, sai da função
    WiFi.begin(SSID, PASSWORD); // Tenta conectar ao WiFi
    while (WiFi.status() != WL_CONNECTED) {
        delay(100); // Aguarda 100 ms
        Serial.print("."); // Imprime ponto na serial enquanto espera
    }
    Serial.println();
    Serial.println("Conectado com sucesso na rede ");
    Serial.print(SSID);
    Serial.println("IP obtido: ");
    Serial.println(WiFi.localIP()); // Imprime o IP obtido
}

void VerificaConexoesWiFIEMQTT() {
    if (!MQTT.connected())
        reconnectMQTT(); // Tenta reconectar ao broker MQTT se desconectado
    reconectWiFi(); // Verifica a conexão WiFi
}

void reconnectMQTT() {
    while (!MQTT.connected()) {
        Serial.print("* Tentando se conectar ao Broker MQTT: ");
        Serial.println(BROKER_MQTT);
        if (MQTT.connect(ID_MQTT)) {
            Serial.println("Conectado com sucesso ao broker MQTT!");
            MQTT.subscribe(TOPICO_SUBSCRIBE); // Assina o tópico MQTT
        } else {
            Serial.println("Falha ao reconectar no broker.");
            Serial.println("Haverá nova tentativa de conexão em 2s");
            delay(2000); // Aguarda 2 segundos antes de tentar novamente
        }
    }
}

// Função para leitura do valor de tensão coletado no pino D34
void handleTemp() {
    const int potPin = 34; // Pino onde o sensor está conectado
    int sensorValue = analogRead(potPin); // Lê o valor do pino analógico
    float temperatura = sensorValue * (100.0 / 4095) + 7; // Converte o valor lido para temperatura
    String mensagem = String(temperatura); // Converte a temperatura para string
    Serial.print("Valor da luminosidade: ");
    Serial.println(mensagem.c_str());
    MQTT.publish(TOPICO_PUBLISH_2, mensagem.c_str()); // Publica a temperatura no tópico MQTT
}
```

## Simulação 

Simulação feita na plataforma WOKWI, utilizando o componente ESP32, o codigo já disponibilizado e um regulador para alterar o valor de temperatura.

![image](https://github.com/Hugo-Victorr/PBL-Project/assets/105120915/bd9e3e4a-1355-4550-94cd-2b07c2c46f38)

## Montagem do sistema

Sistema montado utilizando protoboard, gerador de sinal, ESP32, Sensor de temperatura DHT11,  resistores, jumpers e um motor utilizado para gerar calor dentro de um cubo de acrilico.

![dispositivo](https://github.com/Hugo-Victorr/PBL-Project/assets/105120915/80945669-7f37-4c56-a8b4-6b874df5a6ca)

### Monitoramento pela plataforma desktop

Temperatura estavel em 40 graus: 

![grafico](https://github.com/Hugo-Victorr/PBL-Project/assets/105120915/82c4a63b-a4d9-48f2-bdf4-2f84e1a2f00c)

## Monitoramento ASP.NET 

Temperatura estavel em 36 graus e proxima do SetPoint (35), exibindo erro relativo de -1:

![PBL grafico](https://github.com/Hugo-Victorr/PBL-Project/assets/105120915/ee008902-f7d5-4360-ac83-54b233c6a046)

## Conclusão

Projeto multidisciplinar, desenvolvido utilizando ESP32 como disposiivo IOT, executando comunicação com a lataforma de Back-End FIWARE e através da aplicação projetada em ASP.NET Core 3.1, é possivel consumir as APIs para efetuar o CRUD do dispositivo na plataforma FIWARE e recuperando os dados através do STH-Comet, para plotar o grafico de Temperatura/tempo e exibir as leituras em uma tabela em tempo real. O projeto é embasado na representação em bloco de malaha aberta, função de transferencia em malha fechada e calculo do erro relativo. Para apresentar uma maior precisão no sistema termico do projeto, temos como base os calculos de transferencia de temperatura por condução e convecção levando em consideração as resistencias termicas do ar, do acrilico, das aletas e tendo como variavel de pertubação um ventilador.

## Considerações 

Projeto desenvolvido para o PBL (Project Based Learning), Faculdade Engenheiro Salvador Arena, proposto pelas disciplinas: 

- Sistemas Embarcados (Prof. Fabio Cabrini);
- Controle e Automação (Prof. Marcones Cleber Brito);
- Fenômenos de Transporte (Prof. Ricardo Calvo);
- Linguagem de Progamação I (Prof. Eduardo Rosalem).

## Integrantes

- Danilo Miranda - 081220021- linkedIn: https://www.linkedin.com/in/tognettidm/
- Hugo Victor Lima - 081220009 - linkedIn:  https://www.linkedin.com/in/hugo-victor-lima-9b5046247/
- Matheus Martins - 081220026 - linkedIn: https://www.linkedin.com/in/matheus-martins-70b955196/
- Matheus Pedroza - 081220002 - linkedIn: https://www.linkedin.com/in/matheus-pedroza/
- Thiago Souza - 081220013 - linkedIn: https://www.linkedin.com/in/thiagocicero/

