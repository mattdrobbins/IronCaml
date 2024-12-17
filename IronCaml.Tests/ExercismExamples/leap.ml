let divisible_by_four y = y mod 4 = 0

let divisible_by_hundred y = y mod 100 = 0

let divisible_four_hundred y = y mod 400 = 0

let leap_year y = divisible_by_four y && (not (divisible_by_hundred y) || divisible_four_hundred y)