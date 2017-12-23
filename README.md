# DeckBuilder

Fetches card prices for Pokemon trading card game from multiple stores and compare them.


### Adding a new collection

* Open data.csv file and paste information for the new cards in the first three columns.

 * A good source is Pokellector http://www.pokellector.com/sets/CNV-Crimson-Invasion?list_display=list

 * Be careful with dashes and special characters.

* Run the LigaMagicDownloader to download new indexes for ecom cards

* To fill ecom column, we run LigaMagicMerger.

* Add the new collection to dbcollections.json
