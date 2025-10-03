Voor dit project hanteren we een Git Flow-achtige branchingstrategie:

main

Stabiele branch, bevat altijd de laatste vrijgegeven versie.

Alleen via pull requests gemerged vanuit develop, feature/* of hotfix/*.

develop

Werkbranch voor de sprint. Nieuwe features worden hier geïntegreerd en getest voordat ze naar main gaan.

feature/*

Branches voor nieuwe functionaliteit.


Worden na afronding via een pull request gemerged in develop.

hotfix/*

Branches voor bugfixes of ontbrekende implementaties in stabiele code.


Worden direct naar dev gemerged.
