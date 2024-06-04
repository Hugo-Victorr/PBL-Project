# PBL-Project Based Learning

# CÓDIGO IMPLEMENTADO NO ESP32 PARA O MONITORAMENTO DA TEMPERATURA EM UMA ESTUFA

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

