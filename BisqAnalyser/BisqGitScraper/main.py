import json
import requests
import os.path
from os import system, name, path
from termcolor import colored

markedAddrs = []
tagDict = {}
searchTerms = ["BSQ address:", "BSQ Address:", "bsq address:", "bsq Address:"]

def clear():
    if name == "nt":
        system("cls") # Windows Clear
    else:
        system("clear") # Linux/Mac Clear

def checkIssue(issue):
    username = issue["user"]["login"]
    body = issue["body"]
    number = issue["number"]
    if any(term in body for term in searchTerms) and "B1" in body:
        addrIndex = body.find("B1")
        bsqAddress = body[addrIndex: addrIndex + 35]
        if not(bsqAddress in markedAddrs):
            print("#" + str(number) + "-" + username)
            bodyLen = len(body)
            if bodyLen < 500:
                print(body[0:bodyLen] + "...")
            else:
                print(body[0:500] + "...")
            print(colored(username, "green"))
            print(colored(bsqAddress, "green"))
            input("Press Enter To Confirm...")
            markedAddrs.append(bsqAddress)
            if username in tagDict.keys():
                tagDict[username].append(bsqAddress)
            else:
                tagDict[username] = [bsqAddress]
            clear()

def getCompRequests():
    print("NOTE: while not using GitHub account, only 60 requests allowed per hour")
    url = "https://api.github.com/repos/bisq-network/compensation/issues"
    params = {'state':'closed', 'page':'0'}
    validResponse = True
    pageCounter = 0
    while validResponse == True:
        pageCounter += 1
        params["page"] = pageCounter
        response = requests.get(url, params).json()
        if len(response) == 0:
            validResponse = False
        else:
            filename = "Page" + str(pageCounter) + ".txt"
            with open(filename, "w", encoding="utf-8") as file:
                file.write(json.dumps(response))
            print("Saved Page " + str(pageCounter))

def loadCompRequests():
    clear()
    fileValid = True
    counter = 0
    while fileValid == True:
        counter += 1
        filename = "Page" + str(counter) + ".txt"
        if path.exists(filename):
            with open(filename, "r") as file:
                data = json.load(file)
                for issue in data:
                    checkIssue(issue)
        else:
            fileValid = False
    print(json.dumps(tagDict, indent = 4))

print("Bisq Git Scraper v0.0")
command = ""
while("close" != command):
    command = input("input command: ")
    if "get-data" == command:
        getCompRequests()
    elif "load-data" == command:
        loadCompRequests()
    elif "clear" == command:
        clear()
    elif "close" != command:
        print("Not a valid command")