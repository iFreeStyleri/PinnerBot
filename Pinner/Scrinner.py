# coding=utf-8
from random import randint

from pinscrape import pinscrape
import os
import shutil

def get_image_url(query: str, directory : str, count):
    try:

        print(f"Дядя работает: {query}")

        if os.path.exists(directory):
            shutil.rmtree(directory)
        os.makedirs(directory, exist_ok=True)
        details = pinscrape.scraper.scrape(query, directory, {}, 1, count)

        print(f"Дядя отработал: {details}")

        if details["isDownloaded"] and details['urls_list']:
            image_url = details['urls_list']
            print(f"Урлы: {image_url}")
            return image_url
        else:
            print("Ничего нету.")
            return None
    except Exception as e:
        print(f"Ошибочка вышла: {e}")
        return None