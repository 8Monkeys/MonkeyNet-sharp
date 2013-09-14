MonkeyNet
=========

At the moment, this is just a library to do some socket IO with UDP packets. We can read and write asynchronously to a local socket that is bound to the specified local endpoint or IPv6Any on port 42337.
In order to get notified when messages arrive, you implement the IApplication interface in a class, and register this class to the MessageRouter instance you created before.

The next thing that will get work is the separation of sending and receiving APIs and the IApplication interface will be dropped. Additional information of incoming packets will get provided and some abstraction
to easily read from and write to packets will get implemented.

You can find issues and current state of development at https://8monkeys.atlassian.net/browse/NET