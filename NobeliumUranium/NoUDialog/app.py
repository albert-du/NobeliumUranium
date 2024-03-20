"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""
from chatterbot import ChatBot
from chatterbot.trainers import ChatterBotCorpusTrainer, ListTrainer
import threading
import sys
import time
import json
from flask import Flask
from flask import request
app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app

trainerbot = ChatBot(
        'Nobelium Uranium',
        logic_adaptors=[
            'chatterbot.logic.BestMatch',
            'chatterbot.logic.MathmaticalEvaluation'
        ]
    )

listTrainer = ListTrainer(trainerbot)

waitingChatBot = ChatBot(
    'Nobelium Uranium',
    logic_adaptors=[
        'chatterbot.logic.BestMatch',
        'chatterbot.logic.MathmaticalEvaluation'
    ],
    read_only=True
)



def NewChatBot ():
    global waitingChatBot
    toSend = waitingChatBot
    waitingChatBot = ChatBot(
        'Nobelium Uranium',
        logic_adaptors=[
            'chatterbot.logic.BestMatch',
            'chatterbot.logic.MathmaticalEvaluation'
        ],
        read_only=True
    )
    return toSend

chatbots = {}

def remove_bot(key):
    time.sleep(60*60)
    print(f"removing {key}")
    chatbots.pop(key)

@app.route('/')
def hello():
    """Renders a sample page."""
    return "Hello World!"

@app.route("/response", methods = ['POST'])
def response():
    sending_channel = request.form['sender']
    message = request.form['message']
    if sending_channel not in chatbots:
        #create and add a new bot
        chatbots[sending_channel] = NewChatBot()
        #schedule its destruction
        #thread = threading.Thread(target=remove_bot, args=(sending_channel,))
        #thread.start()
    the_bot = chatbots[sending_channel]
    botresponse = the_bot.get_response(message)
    return str(botresponse)

@app.route("/train", methods = ['POST'])
def train():
    try:
        data: list = json.loads(request.form['data'])
        listTrainer.train(data)        
        return 200
    except:
        return 500  

@app.route("/autolearn", methods = ['GET'])
def autoteach():
    global trainerbot
    trainer = ChatterBotCorpusTrainer(trainerbot)
    trainer.train('chatterbot.corpus.english')
    return "yes"

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', 'localhost')
    app.run(HOST, int(sys.argv[1]))
