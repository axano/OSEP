# Install bloodhound
```
sudo apt install bloodhound
sudo pip3 install bloodhound  # to query data
```
# Use
```
sudo service neo4j start
sudo bloodhound
login with neo4j:1234


bloodhound-python -u offsec@prod.corp1.com -d prod.corp1.com  --collectionmethod Default -gc cdc01.prod.corp1.com -ns 192.168.71.70
```
