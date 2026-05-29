import React from 'react';
import { View, Text, StyleSheet, ScrollView, SafeAreaView } from 'react-native';
import { Heart } from 'lucide-react-native';

import SearchBar from '../components/SearchBar';
import SectionHeader from '../components/SectionHeader';
import EmptyState from '../components/EmptyState';
import SearchResultItem from '../components/SearchResultItem';

import useWishlistViewModel from '../viewmodels/useWishlistViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing, radius } from '../constants/spacing';

function getCategoryLabel(categoryId) {
  const labels = {
    musical: 'Musical',
    show: 'Show / Concerto',
    'teatro-negro': 'Teatro Negro',
    urbano: 'Cultura Urbana',
    infantil: 'Infantil',
    terror: 'Terror / Drama',
    ballet: 'Ballet',
  };

  return labels[categoryId] || 'Espetáculo';
}

export default function WishlistScreen({ navigation }) {
  const {
    searchText,
    filteredWishlistEvents = [],
    hasSavedEvents,
    hasFilteredResults,
    handleSearchChange,
    handleEventPress,
    handleRemoveFromWishlist,
  } = useWishlistViewModel(navigation);

  const shouldShowList = hasSavedEvents && hasFilteredResults;
  const shouldShowEmptyState = !hasSavedEvents;
  const shouldShowNoSearchResults = hasSavedEvents && !hasFilteredResults;

  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.page}>
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <View style={styles.header}>
            <Text style={styles.title}>Wishlist</Text>
            <Text style={styles.subtitle}>
              Seus espetáculos salvos em um só lugar.
            </Text>
          </View>

          <View style={styles.searchCard}>
            <SearchBar
              placeholder="Buscar em favoritos"
              value={searchText}
              onChangeText={handleSearchChange}
            />
          </View>

          <View style={styles.resultsSection}>
            <SectionHeader
              title="Favoritos"
              subtitle={
                shouldShowList
                  ? `${filteredWishlistEvents.length} espetáculos salvos`
                  : 'Acompanhe aqui os eventos que você salvou'
              }
            />

            {shouldShowList && (
              <View style={styles.resultsList}>
                {filteredWishlistEvents.map((event) => (
                  <SearchResultItem
                    key={event.id}
                    title={event.title}
                    location={`${event.location} - ${event.city || 'Brasil'}`}
                    category={getCategoryLabel(event.categoryId)}
                    image={event.image}
                    onPress={() => handleEventPress(event)}
                    isFavorite
                    onFavoritePress={() => handleRemoveFromWishlist(event.id)}
                  />
                ))}
              </View>
            )}

            {shouldShowEmptyState && (
              <View style={styles.wishlistEmptyState}>
                <Heart
                  size={54}
                  strokeWidth={1.9}
                  color={colors.peachBurnt}
                />

                <Text style={styles.emptyTitle}>
                  Você ainda não salvou nenhum espetáculo
                </Text>

                <Text style={styles.emptyDescription}>
                  Explore e salve seus favoritos para acompanhá-los depois
                </Text>

                <Text
                  style={styles.emptyAction}
                  onPress={() => navigation.navigate('Search')}
                >
                  Explorar espetáculos
                </Text>
              </View>
            )}

            {shouldShowNoSearchResults && (
              <View style={styles.emptyCard}>
                <EmptyState
                  title="Nenhum favorito encontrado"
                  description="Tente buscar por outro espetáculo, teatro ou cidade."
                  actionLabel="Limpar busca"
                  onActionPress={() => handleSearchChange('')}
                />
              </View>
            )}
          </View>
        </ScrollView>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
    position: 'relative',
  },

  scrollContent: {
    paddingBottom: 155,
  },

  header: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.xl,
    gap: spacing.xs,
  },

  title: {
    fontSize: typography.size.xxl,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.sm,
    color: colors.grayText,
    lineHeight: typography.lineHeight.sm,
  },

  searchCard: {
    marginTop: spacing.xl,
    marginHorizontal: spacing.md,
    padding: spacing.sm,
    borderRadius: radius.lg,
    backgroundColor: 'rgba(255, 255, 255, 0.94)',
    borderWidth: 1,
    borderColor: colors.grayLight,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.05,
    shadowRadius: 12,
    elevation: 3,
  },

  resultsSection: {
    paddingHorizontal: spacing.md,
    marginTop: spacing.lg,
    gap: spacing.md,
  },

  resultsList: {
    width: '100%',
    borderRadius: radius.lg,
    backgroundColor: colors.white,
    paddingHorizontal: spacing.md,
    borderWidth: 1,
    borderColor: colors.grayLight,
    overflow: 'hidden',
  },

  emptyCard: {
    borderRadius: radius.lg,
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    overflow: 'hidden',
  },

  wishlistEmptyState: {
    minHeight: 360,
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.xxxl,
  },

  emptyTitle: {
    marginTop: spacing.md,
    fontSize: typography.size.md,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    textAlign: 'center',
  },

  emptyDescription: {
    maxWidth: 245,
    marginTop: spacing.sm,
    fontSize: typography.size.xs,
    lineHeight: typography.lineHeight.xs,
    color: colors.grayText,
    textAlign: 'center',
  },

  emptyAction: {
    marginTop: spacing.xxl,
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.purplePrimary,
    textDecorationLine: 'underline',
  },
});
