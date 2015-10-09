using System;
using System.Collections;
using System.Collections.Generic;

public class IntegerMap {
	private List<int?> resourceStock;
	
	public IntegerMap() {
		resourceStock = new List<int?>(100);
		//FIXME may not be necessary for c# to initialize the list... 
		// Check if list[5] returns something when the length is less than 5
		for(int i = 0; i < 100 + 1; i++) {
			resourceStock.Add(0);
		}
	}
	
	public int get(int index) {
		checkIndex(index);
		if(index > resourceStock.Count) {
			return 0;
		}
		int? stock = resourceStock[index];
		
		if(stock == null) stock = 0;
		return stock.Value;
	}
	
	public void put(int index, int amount) {
		checkIndex(index);
		resourceStock[index] = amount;
	}
	
	public void add(int index, int amount) {
		checkIndex(index);
		int stock = get(index);
		if(amount > 0) {
			resourceStock[index] = stock + amount;
		} else {
			if(stock < amount) {
				// SEVERE, resource stock going negative
			} else {
				resourceStock[index] = stock + amount;
			}
		}
	}
	
	public List<int?> list() {
		return resourceStock;
	}
	
	private void checkIndex(int index) {
		if(index >= resourceStock.Count) {
			while(resourceStock.Count <= index) {
				resourceStock.Add(0);
			}
		}
	}
}

