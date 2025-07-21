# FastTech Foods Ordering API

### Descrição
> É uma aplicação em arquitetura de microsserviço, utilizando o Kubernetes para gerenciamento de conteinerização, orquestração e escalabilidade.


<details>
  <summary><strong>Inicializar os pods</strong></summary>
  
### Criar o Micro serviço
  ```
  kubectl apply -f .\k8s
  ```

** Demais serviços de insfraestrutura estão configurados na API Gateway

</details>

<details>
  <summary><strong>Comandos básicos de Kubernetes</strong></summary>

  ### Visualizar
  ```
  kubectl get secrets

  kubectl get pv,pvc

  kubectl get pods,deployment,svc
  
  kubectl get deployment,svc -l app=contact-api
  
  kubectl describe deployment/api-gateway
  
  kubectl logs pods/contact-persistence-9b887cd7d-htr5r --tail=50
  ```
  
  ### Interação
  ```
  kubectl apply -f deployment.yaml
  
  kubectl delete deployment/api-gateway
  
  kubectl delete deployment,svc -l app=contact-api

  kubectl delete configmaps --all
  
  # Editar sem rebuildar a imagem
  kubectl edit configmap api-gateway-config
  
  kubectl rollout restart deployment api-gateway
  ```
</details>
