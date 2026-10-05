import json
with open('.project/STATE.json') as f:
    s = json.load(f)
s['current_task'] = 'P06-T003'
s['status'] = 'READY'
s['blocked'] = False
with open('.project/STATE.json', 'w') as f:
    json.dump(s, f, indent=2)
print('updated')
