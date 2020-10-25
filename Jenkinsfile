pipeline {
    agent any
    environment {
        MAJOR_VERSION = '0'
        MINOR_VERSION = '1'
        IMAGE_TAG = "sandbox/simple-adventure:${env.MAJOR_VERSION}.${env.MINOR_VERSION}.${env.BUILD_NUMBER}"
    }
    stages {
        stage('Build image') {
          steps{
            script {
              dockerImage = docker.build IMAGE_TAG
            }
          }
        }
        stage('Push image') {
          steps{
            script {
              docker.withRegistry(REGISTRY, REGISTRY_CREDENTIAL ) {
                dockerImage.push()
              }
            }
          }
        }
        stage('Cleanup') {
          steps{
            sh "docker rmi $IMAGE_TAG"
          }
        }
    }
}