
from random import randint

from pinscrape import pinscrape
import os
import shutil

def get_image_url(query: str):
    try:
        output_folder = "output"


        if os.path.exists(output_folder):
            shutil.rmtree(output_folder)
        os.makedirs(output_folder, exist_ok=True)
        counter = randint(1, 2)
        details = pinscrape.scraper.scrape(query, output_folder, {}, 1, counter)


        if details["isDownloaded"] and details['urls_list']:
            image_url = details['urls_list'][0]
            return image_url
        else:
            return None
    except Exception as e:
        return None