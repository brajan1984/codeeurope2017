## Modyfikacja konfiguracji na gorąco
W celu przeładowania zmodyfikowanego pliku konfiguracyjnego prometheus.yml lub config.yml (alert manager) należy wysłać żadanie POST do prometheusa lub alert managera np.
```
curl -X POST http://localhost:9090/-/reload