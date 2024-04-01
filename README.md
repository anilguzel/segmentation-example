
# Customer Segmentation Example Project

## Project Description
The Customer Segmentation and Interaction System is a software solution designed to streamline the process of customer segmentation and interaction based on predefined marketing campaigns.

## Architecture

![Architecture](https://github.com/anilguzel/segmentation-example/blob/master/segmentation-example.drawio.svg)

This diagram illustrates the overall architecture of the project.

## Installation and Running

The project can be run on a local machine or a server. Docker is required to run the project.

1. Clone this repository:
    ```bash
    git clone https://github.com/anilguzel/segmentation-example.git
    ```

2. Navigate to the project directory:
    ```bash
    cd project
    ```

3. Start the project using Docker:
    ```bash
    docker-compose up -d
    ```

This command will run the project in the background and start the necessary containers.

## Usage

Once the project has been successfully started, the system simulates order creation through the order API, where an event is then sent to RabbitMQ. Subsequently, the After-Order service fulfills the necessary requirements.



