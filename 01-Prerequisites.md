# Prérequis

## Visual Studio Code
Lien de téléchargement : https://code.visualstudio.com/Download


## Visual Studio Code Extensions : 
* Service Fabric Reliable Services [https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-service-fabric-reliable-services](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-service-fabric-reliable-services)

* [BONUS] XML Extensions
[https://marketplace.visualstudio.com/items?itemName=DotJoshJohnson.xml](https://marketplace.visualstudio.com/items?itemName=DotJoshJohnson.xml)

## Configuration Service Fabric Reliable Services

Suivre l'article de Michael : [http://www.mfery.com/blog/vs-code-extension-for-service-fabric/](http://www.mfery.com/blog/vs-code-extension-for-service-fabric/)

Récapitulatif de l'installation :

```node
npm install -g yo

npm install -g generator-azuresfjava

npm install -g generator-azuresfcsharp

npm install -g generator-azuresfcontainer

npm install -g generator-azuresfguest
```

## Installation Service Fabric local 

* Windows : [https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-get-started)

## Installation SDK .Net Core

Via chocolatey : 
Installation de choco (vous devez ouvrir votre terminal en mode administrator):

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
```

Installation de la bonne version de .Net Core
```bash
choco install dotnetcore-sdk 
```

Si vous avez déja  installé une version antérieure à la v2.1.401, il vous faudra ajouter '--force' à la commande précédente, ou utiliser la commande d'upgrade.
