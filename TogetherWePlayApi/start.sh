#!/bin/bash

# Construire l'application .NET
echo "Building .NET application..."
dotnet build

# Vérifier si la commande de build a réussi
if [ $? -ne 0 ]; then
  echo "Build failed!"
  exit 1
fi

# Démarrer l'application
echo "Running .NET application..."
dotnet run
