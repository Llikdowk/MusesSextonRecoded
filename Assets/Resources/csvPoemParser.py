import csv
import re
import json
from enum import Enum

class State(Enum):
	INIT = 0
	DEFINING_STRUCTURE = 1
	PARSING_START = 2
	PARSING_VERSES = 3


index_title = 0
index_masculine = -1
index_feminine = -1
index_plural = -1
index_firstperson = -1

current_state = State.INIT
landmarkdata = []
landmark_title = ""
verses_masculine = []
verses_feminine = []
verses_plural = []
verses_firstperson = []

def check_row(row):
	global current_state
	global index_title
	global index_masculine
	global index_feminine
	global index_plural
	global index_firstperson
	global landmarkdata
	global landmark_title
	global verses_masculine
	global verses_feminine
	global verses_plural
	global verses_firstperson

	s = '^'
	s += '^'.join(row)
	s += '$'
	print(s)

	print(">>STATE = ", current_state)

	if current_state == State.INIT:
		match = re.search("\^\^\^\^\^\$", s)
		if match:		
			current_state = State.DEFINING_STRUCTURE

	elif current_state == State.DEFINING_STRUCTURE:
		for i in range(1, len(row)):
			print(row[i])
			if row[i] == "Masculine":
				index_masculine = i
			elif row[i] == "Feminine":
				index_feminine = i
			elif row[i] == "Plural":
				index_plural = i
			elif row[i] == "First Person":
				index_firstperson = i
		print(">>index m:", index_masculine)
		print(">>index f:", index_feminine)
		print(">>index p:", index_plural)
		print(">>index fp:", index_firstperson)
		current_state = State.PARSING_START

	elif current_state == State.PARSING_START:
		if row[0] != "Normal":
			landmark_title = row[0]
			print(">>title:", landmark_title)
		else:
			current_state = State.PARSING_VERSES

	if current_state == State.PARSING_VERSES:
		verses_masculine.append(row[index_masculine])
		verses_feminine.append(row[index_feminine])
		verses_plural.append(row[index_plural])
		verses_firstperson.append(row[index_firstperson])
		if row[0] == "If Third":
			landmarkdata.append( {"LandmarkName":landmark_title, "Masculine":verses_masculine[:], "Feminine":verses_feminine[:], "Plural":verses_plural[:], "FirstPerson":verses_firstperson[:]} )
			verses_masculine = []
			verses_feminine = []
			verses_plural = []
			verses_firstperson = []
			current_state = State.PARSING_START
			

def main():
	i = 0
	with open('Adventure Jam 2016 - Poems.csv', newline='') as csvfile:
		filereader = csv.reader(csvfile, delimiter=',', quotechar='"')
		for row in filereader:
			print(i)
			check_row(row)
			i += 1

main()
text = json.dumps(landmarkdata, indent=4)
with open("csvPoemParserOutput.json", "w") as file:
	file.write(text)
