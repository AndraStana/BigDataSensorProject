package com.company;

import org.eclipse.paho.client.mqttv3.*;

import java.util.UUID;
import java.util.concurrent.Callable;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

public class Main {

    public static void main(String[] args) throws MqttException, InterruptedException {
	// write your code here
        String publisherId = UUID.randomUUID().toString();
        IMqttClient publisher = new MqttClient("tcp://farmer.cloudmqtt.com:10002",publisherId);

        MqttConnectOptions options = new MqttConnectOptions();
        options.setAutomaticReconnect(true);
        options.setCleanSession(true);
        options.setConnectionTimeout(10);
        options.setUserName("spiiemme");
        options.setPassword("lO539O-MDg-J".toCharArray());
        publisher.connect(options);

        CountDownLatch receivedSignal = new CountDownLatch(10);
        publisher.subscribe("XDK110/sensor/data", (topic, msg) -> {
            byte[] payload = msg.getPayload();
            System.out.println(  new String(payload));
            receivedSignal.countDown();
        });
        receivedSignal.await(1, TimeUnit.MINUTES);
    }
}
