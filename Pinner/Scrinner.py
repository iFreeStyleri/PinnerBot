# coding=utf-8
from random import randint

from pinscrape import pinscrape
import os
import shutil

def get_image_url(query: str):
    try:
        output_folder = "output"

        print(f"Дядя работает: {query}")

        if os.path.exists(output_folder):
            shutil.rmtree(output_folder)
        os.makedirs(output_folder, exist_ok=True)
        details = pinscrape.scraper.scrape(query, output_folder, {}, 1, 10)

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