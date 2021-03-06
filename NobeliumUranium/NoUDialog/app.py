"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""
from chatterbot import ChatBot
from chatterbot.trainers import ChatterBotCorpusTrainer
import threading
import sys
import time
from flask import Flask
from flask import request
app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app


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

@app.route("/response/", methods = ['POST'])
def response():
    print("A")
    sending_channel = request.form['sender']
    message = request.form['message']
    if sending_channel not in chatbots:
        #create and add a new bot
        chatbots[sending_channel] = NewChatBot()
        #schedule its destruction
        thread = threading.Thread(target=remove_bot, args=(sending_channel,))
        thread.start()
    the_bot = chatbots[sending_channel]
    botresponse = the_bot.get_response(message)
    print("B")
    return str(botresponse)


@app.route("/autolearn/", methods = ['GET'])
def teach():
    chatbot = ChatBot(
        'Nobelium Uranium',
        logic_adaptors=[
            'chatterbot.logic.BestMatch',
            'chatterbot.logic.MathmaticalEvaluation'
        ]
    )
    trainer = ChatterBotCorpusTrainer(chatbot)
    trainer.train('chatterbot.corpus.english')
    return "yes"

if __name__ == '__main__':
    import os
    HOST = os.environ.get('SERVER_HOST', 'localhost')
    app.run(HOST, int(sys.argv[1]))
