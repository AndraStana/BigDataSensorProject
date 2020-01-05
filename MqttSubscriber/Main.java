
import org.eclipse.paho.client.mqttv3.*;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.util.UUID;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;

public class Main {

    public static CountDownLatch receivedSignal = new CountDownLatch(10);
    public static File fout = new File("sensor.txt");

    public static void main(String[] args) throws MqttException, InterruptedException, IOException {


        System.out.println( "----- The Program has started -----");

	
	
		

        String publisherId = UUID.randomUUID().toString();
        IMqttClient publisher = new MqttClient("tcp://farmer.cloudmqtt.com:10002",publisherId);

        MqttConnectOptions options = new MqttConnectOptions();
        options.setAutomaticReconnect(true);
        options.setCleanSession(true);
        options.setConnectionTimeout(10);
        options.setUserName("spiiemme");
        options.setPassword("lO539O-MDg-J".toCharArray());
        publisher.connect(options);

        publisher.subscribe("XDK110/sensor/data", new MqttMessageListenerImpl());
	
		

        receivedSignal.await(1, TimeUnit.MINUTES);


	  try {
            while (true) {
                Thread.sleep(5 * 1000);
				
				Process process2 = Runtime.getRuntime().exec("hadoop fs -copyFromLocal -f sensor.txt /team10/sensor");
				System.out.println( "----- Wrooooote file to hdfs -----");
            }
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
		


    }

    public static class MqttMessageListenerImpl implements IMqttMessageListener {
        @Override
        public void messageArrived(String topic, MqttMessage msg) throws Exception {
            byte[] payload = msg.getPayload();
            System.out.println(  new String(payload));

            FileOutputStream fos = new FileOutputStream(fout, true);
            OutputStreamWriter osw = new OutputStreamWriter(fos);
                osw.write( new String(payload));
            osw.close();
			
			

            receivedSignal.countDown();
        }
    }
}
